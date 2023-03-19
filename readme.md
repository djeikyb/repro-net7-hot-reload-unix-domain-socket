I added a workaround with logging. The logs demonstrate that the unix domain socket still exists. But since we're checking anyway, I remove the socket so that kestrel won't be bothered by it.

I'm using 7.0.202, and have a global.json created like:

```
dotnet new globaljson --sdk-version 7.0.202 --roll-forward patch
```

I used the the webapi template with minimal apis,
 
```
dotnet new webapi --use-minimal-apis -o webapi
```

then inlined the launch properties to come up with a single file. I added a `ListenUnixSocket` to the kestrel setup.

