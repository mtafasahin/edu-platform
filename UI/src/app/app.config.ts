import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { AutoRefreshTokenService, includeBearerTokenInterceptor, provideKeycloak, UserActivityService, withAutoRefreshToken } from 'keycloak-angular';
import { environment } from '../environments/environment';
import { provideKeycloakAngular } from './services/keycloak.config.ts';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

export const appConfig: ApplicationConfig = {
  providers: [
      provideKeycloakAngular(),
      provideZoneChangeDetection({ eventCoalescing: true }), 
      provideRouter(routes),
      provideHttpClient(withInterceptors([includeBearerTokenInterceptor]))]
};
