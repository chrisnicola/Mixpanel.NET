using System;
using System.Collections.Generic;
using FakeItEasy;
using Machine.Specifications;
using System.Linq;
using FluentAssertions;
using Mixpanel.NET.Events;

namespace Mixpanel.NET.Specs.Unit {
  public class tracker_context {
    Establish that = () => {
      FakeHttp = A.Fake<IMixpanelHttp>();
      A.CallTo(() => FakeHttp.Post(A<string>.That.Matches(x => ValidUriCheck(x)), A<string>.Ignored))
        .Invokes(x => CatchSentParameterData(x.GetArgument<string>(0), x.GetArgument<string>(1)))
        .Returns("1");
      A.CallTo(() => FakeHttp.Post(A<string>.That.Matches(x => !ValidUriCheck(x)), A<string>.Ignored))
        .Returns("0");
      A.CallTo(() => FakeHttp.Get(A<string>.That.Matches(x => ValidUriCheck(x)), A<string>.Ignored))
        .Invokes(x => CatchSentParameterData(x.GetArgument<string>(0), x.GetArgument<string>(1)))
        .Returns("1");
      A.CallTo(() => FakeHttp.Get(A<string>.That.Matches(x => !ValidUriCheck(x)), A<string>.Ignored))
        .Returns("0");
      Token = "Your mixpanel token";
      Proxy = null;
      MixpanelTracker = new MixpanelTracker(Token, FakeHttp);
    };
  
    static bool ValidUriCheck(string location) {
      if (string.IsNullOrWhiteSpace(location)) return false;
      if (!Uri.IsWellFormedUriString(location, UriKind.Absolute)) return false;
      return true;
    }
    
    static void CatchSentParameterData(string uri, string data) {
      SentToUri = new Uri(uri);
      SentData = data.UriParameters()["data"].Base64Decode().ParseEvent();
    }

    protected static IMixpanelHttp FakeHttp;   
    protected static MixpanelTracker MixpanelTracker;

    protected static MixpanelEvent SentData;
    protected static MixpanelEvent Event;
    protected static bool Result;
    protected static Uri SentToUri;
    protected static string Token;
    protected static string Proxy;
  }

  [Behaviors]
  public class a_mixpanel_event_sent
  {
    protected static MixpanelEvent SentData;
    protected static MixpanelEvent Event;
    protected static bool Result;
    protected static Uri SentToUri;
    protected static string Token;
    protected static string Proxy;

    It should_track_successfully = () => Result.ShouldBeTrue();
    It should_send_the_event_name = () => SentData.Event.Should().Be(Event.Event);
    It should_send_the_dictionary_properties = () => 
      SentData.Properties.Where(x => x.Key.ToLower() != "time" && x.Key.ToLower() != "token").Should()
      .Equal(Event.Properties.Where(x => x.Key.ToLower() != "time" && x.Key.ToLower() != "token"));
    It should_send_the_token = () => SentData.Properties["token"].Should().Be(Token);
    It should_send_to_the_mixpanel_tracking_url = () => SentToUri.ToString().ShouldStartWith(Resources.Track(Proxy));
  }

  public class when_sending_tracker_data_using_a_dictionary : tracker_context {
    Establish that = () => {
      Event = new MixpanelEvent("My Event", new Dictionary<string, object> {{"prop1", 0}, {"prop2", "string"}});
    };

    Because of = () => Result = MixpanelTracker.Track(Event.Event, Event.Properties);

    Behaves_like<a_mixpanel_event_sent> a_mixpanel_event_was_sent;
    It should_have_set_the_time_automatically = () => SentData.Properties["time"].As<DateTime>()
      .ShouldBeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
  }

  public class when_sending_tracker_data_using_a_mixpanel_event : tracker_context {
    Establish that = () => {
      Event = new MixpanelEvent("My Event", new Dictionary<string, object> {{"prop1", 0}, {"prop2", "string"}});
    };

    Because of = () => Result = MixpanelTracker.Track(Event);

    Behaves_like<a_mixpanel_event_sent> a_mixpanel_event_was_sent;
  }

  public class when_sending_tracker_data_using_an_object_with_default_naming_conventions : tracker_context {
    Establish that = () => {
      _event = new MyEvent { PropertyOne = 0, PropertyTwoFour = "string" };
      Event = _event.ToMixpanelEvent();
    };

    Because of = () => Result = MixpanelTracker.Track(_event);

    Behaves_like<a_mixpanel_event_sent> a_mixpanel_event_was_sent;

    static MyEvent _event;
  }

  public class when_sending_tracker_data_using_an_object_with_literal_serializatioin : tracker_context {
    Establish that = () => {
      MixpanelTracker = new MixpanelTracker(Token, FakeHttp, new TrackerOptions { LiteralSerialization = true });
      _event = new MyEvent { PropertyOne = 0, PropertyTwoFour = "string" };
      Event = _event.ToMixpanelEvent(true);
    };

    Because of = () => Result = MixpanelTracker.Track(_event);

    Behaves_like<a_mixpanel_event_sent> a_mixpanel_event_was_sent;

    static MyEvent _event;
  }

  public class when_sending_tracker_data_using_a_proxy_url : tracker_context
  {
    Establish that = () => {
      Proxy = "http://mytestproxy.com/";
      MixpanelTracker = new MixpanelTracker(Token, FakeHttp, new TrackerOptions { ProxyUrl = Proxy });
      _event = new MyEvent { PropertyOne = 0, PropertyTwoFour = "string" };
      Event = _event.ToMixpanelEvent();
    };

    Because of = () => Result = MixpanelTracker.Track(_event);

    Behaves_like<a_mixpanel_event_sent> a_mixpanel_event_was_sent;

    static MyEvent _event;
  }

  public class when_sending_data_using_get : tracker_context
  {
    Establish that = () => {
      MixpanelTracker = new MixpanelTracker(Token, FakeHttp, new TrackerOptions { UseGet = true });
      _event = new MyEvent { PropertyOne = 0, PropertyTwoFour = "string" };
      Event = _event.ToMixpanelEvent();
    };

    Because of = () => Result = MixpanelTracker.Track(_event);

    Behaves_like<a_mixpanel_event_sent> a_mixpanel_event_was_sent;
    It should_send_via_the_get_method = () => A.CallTo(() => FakeHttp.Get(A<string>.Ignored, A<string>.Ignored))
      .MustHaveHappened();

    static MyEvent _event;
  }         

  public class when_sending_tracker_data_with_no_timestamp : tracker_context
  {
    Establish that = () => {
      MixpanelTracker = new MixpanelTracker(Token, FakeHttp, new TrackerOptions { SetEventTime  = false });
      _event = new MyEvent { PropertyOne = 0, PropertyTwoFour = "string" };
      Event = _event.ToMixpanelEvent();
    };

    Because of = () => Result = MixpanelTracker.Track(_event);

    Behaves_like<a_mixpanel_event_sent> a_mixpanel_event_was_sent;
    It should_have_no_time_value = () => SentData.Properties.Where(x => x.Key.ToLower() == "time").ShouldBeEmpty();

    static MyEvent _event;
  }

  class MyEvent {
    public int PropertyOne { get; set; }
    public string PropertyTwoFour { get; set; }
  }
}