import WidgetKit

@_cdecl("ReloadWidgets")
public func ReloadWidgets ()  {
    WidgetCenter.shared.reloadAllTimelines() 
}