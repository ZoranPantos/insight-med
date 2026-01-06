import { Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { MainLayoutComponent } from './layout/main-layout.component';
import { ReportsComponent } from './reports/reports.component';
import { RequestsComponent } from './requests/requests.component';
import { PatientsComponent } from './patients/patients.component';
import { PatientDetailsComponent } from './patients/patient-details.component';
import { ProfileComponent } from './profile/profile.component';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { 
    path: '', 
    component: MainLayoutComponent, 
    children: [
      { path: 'reports', component: ReportsComponent },
      { path: 'requests', component: RequestsComponent },
      { path: 'patients', component: PatientsComponent },
      { path: 'patients/:id', component: PatientDetailsComponent },
      { path: 'profile', component: ProfileComponent }
    ]
  }
];