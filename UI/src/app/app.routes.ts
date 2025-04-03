import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { ProtectedComponent } from './pages/protected/protected.component';
import { canActivateAuthRole } from './services/auth.guard';

export const routes: Routes = [
    { path: 'login', component: LoginComponent },
    {
      path: 'protected',
      component: ProtectedComponent,
      canActivate: [canActivateAuthRole],
    },
    { path: '', redirectTo: '/protected', pathMatch: 'full' },
  ];
