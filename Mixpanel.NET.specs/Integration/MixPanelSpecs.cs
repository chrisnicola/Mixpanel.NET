using System.Collections.Generic;
using Machine.Specifications;

namespace Mixpanel.NET.specs.Integration
{
  // Use this test to verify your mixpanel token is working.
  public class when_sending_valid_data_to_mixpanel
  {
    Establish that = () =>
    {
      _panel = new MixPanel("Your mixpanel token here");
    };

    Because of = () =>
    {
      var properties = new Dictionary<string, object> { {"prop1", 0}, {"prop2", "tessdfasdfasdfasdfasdft"} };
      _result = _panel.Track("Test", properties, true);
    };

    It should_track_successfully = () => _result.ShouldBeTrue();

    static MixPanel _panel;
    static bool _result;
  }
}