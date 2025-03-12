import { Component, OnInit } from "@angular/core";
import { ApiService } from "../common/services/api.service";

@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss']
})
export class MainComponent implements OnInit{
  constructor(private apiService: ApiService) { }

  ngOnInit(): void {
    this.apiService.get<any>().subscribe(
      data => console.log('Задачи:', data),
      error => console.error('Ошибка:', error)
    );
  }
}
