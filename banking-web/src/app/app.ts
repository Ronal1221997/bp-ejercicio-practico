import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Header } from './shared/components/header/header';
import { Sidebar} from './shared/components/sidebar/sidebar';

@Component({
  selector: 'app-root',
  standalone: true,
  // Importamos lo que vamos a usar en el HTML
  imports: [RouterOutlet, Header, Sidebar],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App { }