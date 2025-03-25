import { Component, OnInit } from '@angular/core';

interface NoteStyle{
  backgroundColor: string;
  rotation: number;
}

@Component({
  selector: 'app-board',
  standalone: false,
  templateUrl: './board.component.html',
  styleUrl: './board.component.scss'
})
export class BoardComponent implements OnInit{
  notes: string[] = [
    'Заметка 1',
    'Заметка 2',
    'Заметка 3',
    'Заметка 4',
    'Заметка 5',
    'Заметка 6',
    'Заметка 7',
    'Заметка 8',
    'Заметка 9',
    'Заметка 10',
    'Заметка 11',
    'Заметка 12',
    'Заметка 13',
    'Заметка 14',
    'Заметка 15'
  ];

  noteStyles: NoteStyle[] = [];

  styles: NoteStyle[] = [
    { backgroundColor: '#FFEB3B', rotation: -2 },   // Желтый
    { backgroundColor: '#FF8A80', rotation: 3 },      // Розово-красный
    { backgroundColor: '#80D8FF', rotation: -1.5 },   // Голубой
    { backgroundColor: '#CCFF90', rotation: 2 }       // Светло-зеленый
  ];

  ngOnInit(): void {
    // Для каждой заметки случайным образом выбираем стиль из массива styles
    this.noteStyles = this.notes.map(() => {
      const randomIndex = Math.floor(Math.random() * this.styles.length);
      return this.styles[randomIndex];
    });
  }
}
