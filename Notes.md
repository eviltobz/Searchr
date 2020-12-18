# Things to do:

## Be source control aware, and flag matches that are .gitignored, or not added to TFS

## Have "favourite" search paths, that can have a friendly name and be easily selected
eg. All TFS, Connectivity, Database...
yeah, it has the recent list, but i want something more betterer.

## Move more stuff from hardcoded into settings file
and add a UI to tweak it
* file & path openers
* binary file types

## FileOpeners
Should be able to open multiple files in a single instance _or_ multiple instances of an editor
* vim defaults to opening files in a new instance, but a command line could open a new instance with multiple files (dunno about adding to an already-open instance though)
* VsCode defaults to a single instance, so opening new files adds em.
* * Is it possible to force a new instance with the right command line args?
* * You can manually open another instance. Then doing "open in code" will use the last active instance.
Might need to rethink the opener vs multiopener thing. Multiopener is (currently) just diffs, and is special-case compared to files, with very limited numbers that can be opened together. Mayhap that should just be a DiffOpener, then FileOpener can add some options for single vs multi instancing

## MouseOver on Binary & System files to say what it's looking for?
* current implementation is a differently-skinned checkbox, and that don't seem to have a tooltip property to set.

## Add "Include folder patterns"
* I wanted to search for stuff in "Interop" folders


# .Net Framework 4.8 to .Net 5 Upgrade
From some simple timings, the .Net 5 code was faster for big searches, but the Framework code was faster with a small search where drawing to the screen became a significant part of the run. This may be possible to improve by looking at how results are marshalled back to the UI thread. Or something.

The Framework version had the following in the App.Config to help with long filename stuff:
  <runtime>
    <AppContextSwitchOverrides value="Switch.System.IO.UseLegacyPathHandling=false;Switch.System.IO.BlockLongPaths=false"/>
  </runtime>
The machine I'm now developing on doesn't have long filename support enabled, and the group policy editor doesn't even show the option, so I can't do any tests to see if something like that is still required.


