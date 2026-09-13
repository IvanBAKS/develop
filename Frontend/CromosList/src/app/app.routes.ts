import { Routes } from '@angular/router';
import { Layout } from './layout/layout';
import { LoginComponent } from './pages/login/login';
import { RegistroComponent } from './pages/registro/registro';
import { AnadirCromoComponent } from './pages/cromos/cromos';
import { EquiposComponent } from './pages/equipos/equipos';
import { JugadoresComponent } from './pages/jugadores/jugadores';
import { TiposCromoComponent } from './pages/tipos-cromo/tipos-cromo';
import { AlbumesComponent } from './pages/albumes/albumes';
import { EditorialesComponent } from './pages/editoriales/editoriales';
import { ColeccionDetalleComponent } from './pages/coleccion-detalle/coleccion-detalle';
import { AdminPanelComponent } from './pages/admin-panel/admin-panel';
import { EnProgresoComponent } from './pages/en-progreso/en-progreso';
import { authGuard } from './guards/auth.guard';
import { esAdminGuard } from './guards/es-admin.guard';

export const routes: Routes = [
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'registro',
    component: RegistroComponent
  },
  {
    path: 'en-progreso',
    component: EnProgresoComponent
  },
  {
    path: '',
    component: Layout,
    canActivate: [authGuard],
    children: [
      {
        path: '',
        redirectTo: 'panel',
        pathMatch: 'full'
      },
      {
        path: 'panel',
        component: AdminPanelComponent,
        canActivate: [esAdminGuard]
      },
      {
        path: 'cromos',
        component: AnadirCromoComponent,
        canActivate: [esAdminGuard]
      },
      {
        path: 'equipos',
        component: EquiposComponent,
        canActivate: [esAdminGuard]
      },
      {
        path: 'jugadores',
        component: JugadoresComponent,
        canActivate: [esAdminGuard]
      },
      {
        path: 'tipos-cromo',
        component: TiposCromoComponent,
        canActivate: [esAdminGuard]
      },
      {
        path: 'albumes',
        component: AlbumesComponent,
        canActivate: [esAdminGuard]
      },
      {
        path: 'editoriales',
        component: EditorialesComponent,
        canActivate: [esAdminGuard]
      },
      {
        path: 'album/:id',
        component: ColeccionDetalleComponent,
        canActivate: [esAdminGuard]
      }
    ]
  },
  {
    path: '**',
    redirectTo: ''
  }
];
