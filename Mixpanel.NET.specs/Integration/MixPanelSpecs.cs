using System;
using System.Collections.Generic;
using FakeItEasy;
using Machine.Specifications;

namespace Mixpanel.NET.specs.Integration
{
  // Use this test to verify your mixpanel token is working.
  public class when_sending_valid_dictionary_data_to_mixpanel
  {
    Establish that = () =>
    {
      _panel = new Tracker("Your mixpanel token here");
    };

    Because of = () =>
    {
      var properties = new Dictionary<string, object> { {"prop1", 0}, {"prop2", "tessdfasdfasdfasdfasdft"} };
      _result = _panel.Track("Test", properties, true);
    };

    It should_track_successfully = () => _result.ShouldBeTrue();

    static Tracker _panel;
    static bool _result;
  }

  public class when_sending_a_valid_event_object_to_mixpanel
  {
    Establish that = () =>
    {
      _event = new MyCrazyTestEvent
      {
        Data1 = "Some data here",
        Data2 = "Some more data"
      };
    };

    static MyCrazyTestEvent _event;
  }

  public class MyCrazyTestEvent : TrackingEvent
  {
    public string Data1 { get; set; }

    public string Data2 { get; set; }
  }
}