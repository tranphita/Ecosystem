fetch('/assets/appsettings.json')
  .then((res) => res.json())
  .then((config) => {
    (window as any)['appConfig'] = config;
    import('./app/app.config').then(({ appConfig }) => {
      import('./app/app.component').then(({ AppComponent }) => {
        import('@angular/platform-browser').then(({ bootstrapApplication }) => {
          bootstrapApplication(AppComponent, appConfig).catch((err) =>
            console.error(err)
          );
        });
      });
    });
  });
