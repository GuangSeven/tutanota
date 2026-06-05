## Building and running your own Tuta Mail Desktop client (Windows)

Keep in mind that your own build of Tuta Mail Desktop will not update automatically.

### Pre-requisites:

* An up-to-date version of Git is installed.
* Node.js (check package.json `engines` field for the version)

### Preparations:

0. Open a terminal.
1. Clone the repository: `git clone https://github.com/tutao/tutanota.git`.
2. Switch into the Tutanota directory: `cd tutanota`
3. Checkout the latest web release tag: `git checkout tutanota-release-xxx`
4. Initialize liboqs and argon2 submodules: `git submodule init`
5. Synchronize submodules: `git submodule sync --recursive`
6. Update submodules: `git submodule update`
7. Run `npm ci` to install dependencies.

> If you try building without initializing the submodules, you might end up with
> the following error:
>
> ```
> Build error: Error: Could not load wasm-loader:liboqs.wasm (imported by src/applications/common/api/worker/facades/KyberFacade.ts): Command failed: make -f Makefile_liboqs build
> liboqs/src/kem/kem.c:12:10: fatal error: 'oqs/oqs.h' file not found
> ```
>
> Just delete the `libs/webassembly/include` directory and re-build.

### Build:

1. Run `node desktop --custom-desktop-release`.

The client for your platform will be in `artifacts/desktop/`. Note that you can add `--unpacked` to the build command to
skip the packaging of the installer. This will yield a directory in `build/desktop/` containing the client that can be run without
installation.

### Extra Notes:

The **windows** client uses a native dependency to enable MAPI Support. The source can be found
at https://github.com/tutao/mapirs. You can build it yourself before building the client and the build process will pick
the artifact up automatically if you structure the projects a such:

```
parent dir
├── mapirs
└── tutanota-3
```

Otherwise, the builder will load the current release from https://github.com/tutao/mapirs/releases/latest .