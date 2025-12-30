import { Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { MainLayoutComponent } from './layout/main-layout.component';
import { ReportsComponent } from './reports/reports.component';
import { RequestsComponent } from './requests/requests.component';
import { PatientsComponent } from './patients/patients.component';
import { ProfileComponent } from './profile/profile.component';

export const routes: Routes = [
  // 1. If path is empty, redirect to Login immediately
  { path: '', redirectTo: 'login', pathMatch: 'full' },

  // 2. The Login Page (No Navigation Bar)
  { path: 'login', component: LoginComponent },

  // 3. The Main App Area (Has Navigation Bar)
  { 
    path: '', 
    component: MainLayoutComponent, 
    children: [
      // When we are inside MainLayout, 'reports' loads in its inner <router-outlet>
      { path: 'reports', component: ReportsComponent },
      { path: 'requests', component: RequestsComponent },
      { path: 'patients', component: PatientsComponent },
      { path: 'profile', component: ProfileComponent }
    ]
  }
];