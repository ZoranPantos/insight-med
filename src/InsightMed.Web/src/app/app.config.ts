import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter, withRouterConfig } from '@angular/router'; // 1. Import withRouterConfig
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './core/auth.interceptor';

import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    
    // 2. Add withRouterConfig to enable reloading on same URL
    provideRouter(
      routes, 
      withRouterConfig({ onSameUrlNavigation: 'reload' })
    ),

    // 3. Cleaned up: only one provideHttpClient call is needed
    provideHttpClient(withInterceptors([authInterceptor])),
  ]
};