import { Component, OnInit } from "@angular/core";
import { ApiService } from "../common/services/api.service";

@Component({
  selector: 'app-main',
  standalone: false,
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss']
})
export class MainComponent implements OnInit{
  constructor(
    private apiService: ApiService
  ) { }

  ngOnInit(): void {

  }
}
