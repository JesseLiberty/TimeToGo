# TimeToGo

A small Windows desktop app that tells you what time you can leave once you reach 40 hours for the week.

Enter the time you **Arrived** (for example `8:30 am`) and the hours worked **So Far** this week as a decimal (for example `12.5`), then press **Enter**. **Time to Go** shows, in red, the time you reach 40 hours: arrival time + (40 − hours so far), on a 12-hour clock.

## Build

Requires the .NET 10 SDK on Windows.

```
cd TimeToGo
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o ..\publish
```

The executable is written to `publish\TimeToGo.exe`.

## License

MIT © 2026 Jesse Liberty. See [LICENSE](LICENSE).
