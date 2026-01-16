import { Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { MainLayoutComponent } from './layout/main-layout.component';
import { ReportsComponent } from './reports/reports.component';
import { ReportDetailsComponent } from './reports/report-details.component';
import { RequestsComponent } from './requests/requests.component';
import { PatientsComponent } from './patients/patients.component';
import { AddPatientComponent } from './patients/add-patient.component';
import { PatientDetailsComponent } from './patients/patient-details.component';
import { ProfileComponent } from './profile/profile.component';
import { ChangePasswordComponent } from './profile/change-password.component';
import { CreateRequestComponent } from './requests/create-request.component';
import { ParameterAnalyticsComponent } from './parameters/parameter-analytics.component';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { 
    path: '', 
    component: MainLayoutComponent, 
    children: [
      { path: 'reports', component: ReportsComponent },
      { path: 'reports/:id', component: ReportDetailsComponent },
      { path: 'requests', component: RequestsComponent },
      { path: 'requests/create', component: CreateRequestComponent },
      { path: 'patients', component: PatientsComponent },
      { path: 'patients/add', component: AddPatientComponent },
      { path: 'patients/:id', component: PatientDetailsComponent },
      { path: 'profile', component: ProfileComponent },
      { path: 'change-password', component: ChangePasswordComponent },
      { path: 'patients/:id/analytics', component: ParameterAnalyticsComponent }
    ]
  }
];