using System.Collections.Generic;
using Machine.Specifications;

namespace Mixpanel.NET.Specs.Integration
{
  // Use this test to verify your mixpanel token is working.
  public class when_sending_valid_dictionary_data_to_mixpanel {
    Establish that = () => {
      _panel = new MixpanelTracker("Your mixpanel token here", new TrackerOptions{ Test = true, Bucket = "test" });
      _properties = new Dictionary<string, object> { {"prop1", 0}, {"prop2", "tessdfasdfasdfasdfasdft"} };
    };

    Because of = () => _result = _panel.Track("Test", _properties);

    It should_track_successfully = () => _result.ShouldBeTrue();

    static MixpanelTracker _panel;
    static bool _result;
    static Dictionary<string, object> _properties;
  }

  public class when_sending_a_valid_event_object_to_mixpanel {
    Establish that = () => {
      _panel = new MixpanelTracker("Your mixpanel token here");
      _event = new MyCrazyTestEvent {
        Data1 = "Some data here",
        Data2 = "Some more data"
      };
    };

    Because of = () => _result = _panel.Track(_event);

    It should_track_successfully = () => _result.ShouldBeTrue();

    static MyCrazyTestEvent _event;
    static MixpanelTracker _panel;
    static bool _result;
  }

  public class MyCrazyTestEvent : TrackingEvent {
    public string Data1 { get; set; }

    public string Data2 { get; set; }
  }
}