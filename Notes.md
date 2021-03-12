# Things to do:

## Be source control aware, and flag matches that are .gitignored, or not added to TFS
TFS API info:
* https://stackoverflow.com/questions/38219680/c-sharp-tfs-api-show-project-structure-with-folders-and-files-including-their
* https://docs.microsoft.com/en-us/azure/devops/integrate/concepts/dotnet-client-libraries?view=azure-devops#reference
libgit2 info:
* https://libgit2.org/libgit2/#HEAD/group/ignore/git_ignore_path_is_ignored
* https://github.com/libgit2/libgit2sharp/wiki



## "Special" openers?
Be able to open a VS solution from a search result?
* look in the file's folder, then step back through parent folders looking for the first .sln?
* * or list of .sln files to choose if multiple are found - in same folder, or all up the heirarchy?
* Call it "Project Opener"?
* * have a pattern for the file (or directory) it's looking for, and the commandline to open it with
* * could be used for vscode type things, or other projecty type thingies...
* * * .git folder -> git extensions?

## Have "favourite" search paths, that can have a friendly name and be easily selected
eg. All TFS, Connectivity, Database...
yeah, it has the recent list, but i want something more betterer.
* And the same for other bits, like include patterns, so i can easily restrict it to just source files - .cs, .cls, .bas, .asp... etc.
* Have these things in settings. always put em at the top of the list alphabetically, display as something like [Friendly Name (.this, .that)]

## Have SUPER AWESOME FAVOURITE paths, that it caches & sticks a filewatcher on!!!!
* /checks the size of the TFS folder...
* ok, maybe not :(
* * Maybe investigate a bit more. How big would it be if limited to .sln, .csproj, .cs, .sql .vb and a few others for some standard search stuff?
* * Load up configured cached paths in a background thread at startup - pause it any time an actual search is running - Once it's loaded, stick a filewatcher on it. If you search in that location *with that limitation on filetypes* it'll do it all from rams.

## Move more stuff from hardcoded into settings file
and add a UI to tweak it
* file & path openers
* binary & system file types

## MouseOver on Binary & System files to say what it's looking for?
* current implementation is a differently-skinned checkbox, and that don't seem to have a tooltip property to set.

## Add "Include folder patterns"
* I wanted to search for stuff in "Interop" folders

## Improve the Include/Exclude bits
* They never seem to work when I try to use em, what are they _actually_ doing?
* Do they get applied when running a filter, rather than a search? 


# .Net Framework 4.8 to .Net 5 Upgrade
From some simple timings, the .Net 5 code was faster for big searches, but the Framework code was faster with a small search where drawing to the screen became a significant part of the run. This may be possible to improve by looking at how results are marshalled back to the UI thread. Or something.

The Framework version had the following in the App.Config to help with long filename stuff:
  <runtime>
    <AppContextSwitchOverrides value="Switch.System.IO.UseLegacyPathHandling=false;Switch.System.IO.BlockLongPaths=false"/>
  </runtime>
The machine I'm now developing on doesn't have long filename support enabled, and the group policy editor doesn't even show the option, so I can't do any tests to see if something like that is still required.


