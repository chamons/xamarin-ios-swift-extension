//
//  NativeWidget.swift
//  NativeWidget
//
//  Created by Chris Hamons on 6/30/20.
//

import WidgetKit
import SwiftUI
import Intents

struct Provider: IntentTimelineProvider {
    public func snapshot(for configuration: ConfigurationIntent, with context: Context, completion: @escaping (SimpleEntry) -> ()) {
        let entry = SimpleEntry(date: Date(), configuration: configuration)
        completion(entry)
    }

    public func timeline(for configuration: ConfigurationIntent, with context: Context, completion: @escaping (Timeline<Entry>) -> ()) {
        var entries: [SimpleEntry] = []

        // Generate a timeline consisting of five entries an hour apart, starting from the current date.
        let currentDate = Date()
        for hourOffset in 0 ..< 5 {
            let entryDate = Calendar.current.date(byAdding: .hour, value: hourOffset, to: currentDate)!
            let entry = SimpleEntry(date: entryDate, configuration: configuration)
            entries.append(entry)
        }

        let timeline = Timeline(entries: entries, policy: .atEnd)
        completion(timeline)
    }
}

struct SimpleEntry: TimelineEntry {
    public let date: Date
    public let configuration: ConfigurationIntent
}

struct PlaceholderView : View {
    var body: some View {
        Text("Placeholder View")
    }
}

struct NativeWidgetEntryView : View {
    var entry: Provider.Entry

    var body: some View {
        Text(entry.date, style: .time)
    }
}

@main
struct NativeWidget: Widget {
    private let kind: String = "NativeWidget"

    public var body: some WidgetConfiguration {
        IntentConfiguration(kind: kind, intent: ConfigurationIntent.self, provider: Provider(), placeholder: PlaceholderView()) { entry in
            NativeWidgetEntryView(entry: entry)
        }
        .configurationDisplayName("My Widget")
        .description("This is an example widget.")
    }
}
