using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using FakeItEasy;
using Machine.Specifications;
using System.Linq;

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
      MixpanelTracker = new MixpanelTracker("Your mixpanel token", FakeHttp);
    };
  
    static bool ValidUriCheck(string location) {
      if (string.IsNullOrWhiteSpace(location)) return false;
      if (!Uri.IsWellFormedUriString(location, UriKind.Absolute)) return false;
      return true;
    }
    
    static void CatchSentParameterData(string uri, string data) {
      SentToUri = new Uri(uri);
      SentData = data.UriParameters()["data"].Base64Decode();
    }

    protected static IMixpanelHttp FakeHttp;   
    protected static MixpanelTracker MixpanelTracker;
    protected static string SentData;
    protected static Uri SentToUri;
  }

  public class when_sending_tracker_data_using_a_dictionary : tracker_context {
    Establish that = () => {
      _properties = new Dictionary<string, object> {{"prop1", 0}, {"prop2", "string"}};
      _name = "My Event";
    };

    Because of = () => _result = MixpanelTracker.Track(_name, _properties);

    It should_track_successfully = () => _result.ShouldBeTrue();
    It should_send_the_event_name = () => SentData.ShouldHaveName(_name);
    It should_send_the_dictionary_property_1 = () => 
      SentData.ShouldHaveProperty(_properties.ElementAt(0).Key, _properties.ElementAt(0).Value);
    It should_send_the_dictionary_property_2 = () => 
      SentData.ShouldHaveProperty(_properties.ElementAt(1).Key, _properties.ElementAt(1).Value);
    It should_send_to_the_mixpanel_tracking_url = () => SentToUri.ToString().ShouldStartWith(Resources.Track());

    static bool _result;
    static Dictionary<string, object> _properties;
    static string _name;
  }

  public class when_sending_tracker_data_using_a_mixpanel_event : tracker_context {
    Establish that = () => {
      var properties = new Dictionary<string, object> {{"prop1", 0}, {"prop2", "string"}};
      _event = new MixpanelEvent("My Event", properties);
    };

    Because of = () => _result = MixpanelTracker.Track(_event);

    It should_track_successfully = () => _result.ShouldBeTrue();
    It should_send_the_event_name = () => SentData.ShouldHaveName(_event.Event);
    It should_send_the_dictionary_property_1 = () => 
      SentData.ShouldHaveProperty(_event.Properties.ElementAt(0).Key, _event.Properties.ElementAt(0).Value);
    It should_send_the_dictionary_property_2 = () => 
      SentData.ShouldHaveProperty(_event.Properties.ElementAt(1).Key, _event.Properties.ElementAt(1).Value);
    It should_send_to_the_mixpanel_tracking_url = () => SentToUri.ToString().ShouldStartWith(Resources.Track());

    static bool _result;
    static MixpanelEvent _event;
  }

  public class when_sending_tracker_data_using_an_object_with_default_naming_conventions : tracker_context {
    Establish that = () => _event = new MyEvent { PropertyOne = 0, PropertyTwoFour = "string" };

    Because of = () => _result = MixpanelTracker.Track(_event);

    It should_track_successfully = () => _result.ShouldBeTrue();
    It should_send_the_event_name = () => SentData.ShouldHaveName("My Event");
    It should_send_property_one = () => SentData.ShouldHaveProperty("Property One", _event.PropertyOne);
    It should_send_property_two = () => SentData.ShouldHaveProperty("Property Two Four", _event.PropertyTwoFour);
    It should_send_to_the_mixpanel_tracking_url = () => SentToUri.ToString().ShouldStartWith(Resources.Track());

    static MyEvent _event;
    static bool _result;
  }

  public class when_sending_tracker_data_using_an_object_with_literal_serializatioin : tracker_context {
    Establish that = () => {
      MixpanelTracker = new MixpanelTracker("my token", FakeHttp, new TrackerOptions { LiteralSerialization = true });
      _event = new MyEvent { PropertyOne = 0, PropertyTwoFour = "string" };
    };

    Because of = () => _result = MixpanelTracker.Track(_event);

    It should_track_successfully = () => _result.ShouldBeTrue();
    It should_send_the_event_name = () => SentData.ShouldHaveName("MyEvent");
    It should_send_property_one = () => SentData.ShouldHaveProperty("PropertyOne", _event.PropertyOne);
    It should_send_property_two = () => SentData.ShouldHaveProperty("PropertyTwoFour", _event.PropertyTwoFour);
    It should_send_to_the_mixpanel_tracking_url = () => SentToUri.ToString().ShouldStartWith(Resources.Track());

    static MyEvent _event;
    static bool _result;
  }

  public class when_sending_tracker_data_using_a_proxy_url : tracker_context
  {
    Establish that = () => {
      _proxy = "http://mytestproxy.com/";
      MixpanelTracker = new MixpanelTracker("my token", FakeHttp, new TrackerOptions { ProxyUrl = _proxy });
      _event = new MyEvent {
        PropertyOne = 0,
        PropertyTwoFour = "string"
      };
    };

    Because of = () => _result = MixpanelTracker.Track(_event);

    It should_track_successfully = () => _result.ShouldBeTrue();
    It should_send_the_event_name = () => SentData.ShouldHaveName("My Event");
    It should_send_property_one = () => SentData.ShouldHaveProperty("Property One", _event.PropertyOne);
    It should_send_property_two = () => SentData.ShouldHaveProperty("Property Two Four", _event.PropertyTwoFour);
    It should_send_to_the_proxy_url = () => SentToUri.ToString().ShouldStartWith(Resources.Track(_proxy));

    static MyEvent _event;
    static bool _result;
    static string _proxy;
  }

  public class when_sending_data_using_get : tracker_context
  {
    Establish that = () => {
      MixpanelTracker = new MixpanelTracker("my token", FakeHttp, new TrackerOptions { UseGet = true });
      _event = new MyEvent {
        PropertyOne = 0,
        PropertyTwoFour = "string"
      };
    };

    Because of = () => _result = MixpanelTracker.Track(_event);

    It should_track_successfully = () => _result.ShouldBeTrue();
    It should_send_the_event_name = () => SentData.ShouldHaveName("My Event");
    It should_send_property_one = () => SentData.ShouldHaveProperty("Property One", _event.PropertyOne);
    It should_send_property_two = () => SentData.ShouldHaveProperty("Property Two Four", _event.PropertyTwoFour);
    It should_send_via_the_get_method = () => A.CallTo(() => FakeHttp.Get(A<string>.Ignored, A<string>.Ignored))
      .MustHaveHappened();

    static MyEvent _event;
    static bool _result;
  }         

  class MyEvent {
    public int PropertyOne { get; set; }
    public string PropertyTwoFour { get; set; }
  }

  public static class ShouldExtenstions
  {
    public static T ShouldBeJsonOf<T>(this string source)
    {
      return new JavaScriptSerializer().Deserialize<T>(source);
    }
    
    public static void ShouldHaveName(this string source, string name)
    {
      var data = source.ParseEvent();
      if (!name.Equals(data.Event))
        throw new SpecificationException("Event name did not match");
    }
    public static void ShouldHaveProperty(this string source, string name, object value)
    {
      var data = source.ParseEvent();
      if (!value.Equals(data.Properties[name]))
        throw new SpecificationException("Property value did not match");
    }
  }
}