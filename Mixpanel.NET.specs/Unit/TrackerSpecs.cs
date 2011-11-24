using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using FakeItEasy;
using Machine.Specifications;

namespace Mixpanel.NET.Specs.Unit {
  public class tracker_context {
    Establish that = () => {
      FakeHttp = A.Fake<IMixpanelHttp>();
      A.CallTo(() => FakeHttp.Get(A<string>.That.Matches(x => ValidUriCheck(x))))
        .Invokes(x => CatchSentParameterData(x.GetArgument<string>(0)))
        .Returns("1");
      A.CallTo(() => FakeHttp.Get(A<string>.That.Matches(x => !ValidUriCheck(x))))
        .Returns("0");
      Tracker = new Tracker("Your mixpanel token", FakeHttp);
    };
  
    static bool ValidUriCheck(string location) {
      if (string.IsNullOrWhiteSpace(location)) return false;
      if (!Uri.IsWellFormedUriString(location, UriKind.Absolute)) return false;
      return true;
    }
    
    static void CatchSentParameterData(string data) {
      SentToUri = new Uri(data);
      SentData = data.UriParameters()["data"].Base64Decode();
    }

    protected static IMixpanelHttp FakeHttp;   
    protected static Tracker Tracker;
    protected static string SentData;
    protected static Uri SentToUri;
  }

  public class when_sending_tracker_data_using_a_dictionary : tracker_context {
    Because of = () => {
      var properties = new Dictionary<string, object> {{"prop1", 0}, {"prop2", "string"}};
      _result = Tracker.Track("Test", properties);
    };

    It should_track_successfully = () => _result.ShouldBeTrue();
    It should_send_the_event_name = () => SentData.ShouldHaveName("Test");
    It should_send_the_dictionary_property_1 = () => SentData.ShouldHaveProperty("prop1", 0);
    It should_send_the_dictionary_property_2 = () => SentData.ShouldHaveProperty("prop2", "string");
    It should_send_to_the_mixpanel_tracking_url = () => SentToUri.ToString().ShouldStartWith(Resources.Track());

    static bool _result;
  }

  public class when_sending_tracker_data_using_an_object_with_default_naming_conventions : tracker_context {
    Because of = () => {
      _event = new MyEvent {
        PropertyOne = 0, PropertyTwoFour = "string"
      };
      _result = Tracker.Track(_event);
    };

    It should_track_successfully = () => _result.ShouldBeTrue();
    It should_send_the_event_name = () => SentData.ShouldHaveName("My Event");
    It should_send_property_one = () => SentData.ShouldHaveProperty("Property One", _event.PropertyOne);
    It should_send_property_two = () => SentData.ShouldHaveProperty("Property Two Four", _event.PropertyTwoFour);
    It should_send_to_the_mixpanel_tracking_url = () => SentToUri.ToString().ShouldStartWith(Resources.Track());

    static MyEvent _event;
    static bool _result;
  }

  public class when_sending_tracker_data_using_an_object_with_literal_serializatioin : tracker_context {
    Because of = () => {
      Tracker = new Tracker("my token", FakeHttp, new TrackerOptions { LiteralSerialization = true });
      _event = new MyEvent {
        PropertyOne = 0, PropertyTwoFour = "string"
      };
      _result = Tracker.Track(_event);
    };

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
      Tracker = new Tracker("my token", FakeHttp, new TrackerOptions { ProxyUrl = _proxy });
      _event = new MyEvent {
        PropertyOne = 0,
        PropertyTwoFour = "string"
      };
    };

    Because of = () => _result = Tracker.Track(_event);

    It should_track_successfully = () => _result.ShouldBeTrue();
    It should_send_the_event_name = () => SentData.ShouldHaveName("My Event");
    It should_send_property_one = () => SentData.ShouldHaveProperty("Property One", _event.PropertyOne);
    It should_send_property_two = () => SentData.ShouldHaveProperty("Property Two Four", _event.PropertyTwoFour);
    It should_send_to_the_proxy_url = () => SentToUri.ToString().ShouldStartWith(Resources.Track(_proxy));

    static MyEvent _event;
    static bool _result;
    static string _proxy;
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