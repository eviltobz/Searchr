# Things to do:

## Be source control aware, and flag matches that are .gitignored, or not added to TFS

## Have "favourite" search paths, that can have a friendly name and be easily selected
eg. All TFS, Connectivity, Database...
yeah, it has the recent list, but i want something more betterer.

## Move more stuff from hardcoded into settings file
and add a UI to tweak it
* file & path openers
* binary file types

# .Net Framework 4.8 to .Net 5 Upgrade
From some simple timings, the .Net 5 code was faster for big searches, but the Framework code was faster with a small search where drawing to the screen became a significant part of the run. This may be possible to improve by looking at how results are marshalled back to the UI thread. Or something.

The Framework version had the following in the App.Config to help with long filename stuff:
  <runtime>
    <AppContextSwitchOverrides value="Switch.System.IO.UseLegacyPathHandling=false;Switch.System.IO.BlockLongPaths=false"/>
  </runtime>
The machine I'm now developing on doesn't have long filename support enabled, and the group policy editor doesn't even show the option, so I can't do any tests to see if something like that is still required.


