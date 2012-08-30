## Mixpanel.NET API Integration for .NET

This project provides basic API integration for using Mixpanel from .NET
applications.  I'm specifically building it with desktop analytics in mind but
it should be useful for a wide range of analytics tracking scenarios.

If you have feature suggestions please open a github issue for them.

### Current Features:

* Make basic calls to http://api.mixpanel.com/track and track events

### Planned Features:

* Seperate API wrappers for the Mixpanel data query API

### Usage:

First, I recommend looking at the specs since they pretty clearly demonstrate
the usage, scenarios and options.  To get started with most of the sensible
default options first create a new `Tracker`

```csharp
var tracker = new Tracker("your API key");
```

Tracking data can be done one of two ways, first, by sending properties in a
dictionary.

```csharp
var properties = new Dictionary<string, object>();
properties["something"] = "some data";
properties["something else"] = 5;
properties["time"] = DateTime.Now;
tracker.Track("My Event", properties);
```

Alternatively, you can use a class which will have it's name and properties
serialzied.

```csharp
var trackingEvent = new MyEvent {
  Something = "some data",
  SomethingElse = 5,
  Time = DateTime.Now
};
tracker.Track(trackingEvent);
```

Because it is conventional for analytics tracking, all camel-case names are
split with spaces inserted between words.  This makes the reporting easier to
understand.

If you want to serialize the names literally then you can set that by passing a
`TrackerOptions` class with `LiteralSerialization = true` in the constructor of
your `Tracker`.

### Licence

[Licenced under the Apache 2.0 licence](https://github.com/lucisferre/Mixpanel.NET/blob/master/licence.txt)
