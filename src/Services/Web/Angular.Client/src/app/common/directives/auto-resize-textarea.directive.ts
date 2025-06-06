import { Directive, ElementRef, HostListener, Input, OnChanges, OnInit, Renderer2, SimpleChanges } from '@angular/core';

@Directive({
  selector: '[appAutoResizeTextarea]',
  standalone: false
})
export class AutoResizeTextareaDirective implements OnInit, OnChanges {
  // Максимальное количество строк, до которого будет расширяться textarea
  maxRows: number = 5;
  // Высота одной строки (в пикселях)
  lineHeight!: number;
  // Исходная высота textarea, вычисленная из атрибута rows (обычно 1 строка)
  initialHeight!: number;

  @Input() autoResizeValue!: string;

  constructor(private elementRef: ElementRef, private renderer: Renderer2) { }

  ngOnInit(): void {
    const textarea: HTMLTextAreaElement = this.elementRef.nativeElement;
    const computedStyle = window.getComputedStyle(textarea);

    // Получаем значение line-height. Если оно не числовое (например "NaN"), ориентируемся на размер шрифта.
    let lh = parseFloat(computedStyle.lineHeight);
    if (isNaN(lh)) {
      lh = parseFloat(computedStyle.fontSize) * 1.2;
    }
    this.lineHeight = lh;

    // Получаем количество строк, заданное атрибутом rows (если не задан, по умолчанию 1)
    const attrRows = textarea.getAttribute('rows');
    const initialRows = attrRows ? Number(attrRows) : 1;
    this.initialHeight = this.lineHeight * initialRows;

    // Устанавливаем исходную высоту и скрываем скролл
    this.renderer.setStyle(textarea, 'height', `${this.initialHeight}px`);
    this.renderer.setStyle(textarea, 'overflow-y', 'hidden');
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['autoResizeValue']) {
      this.adjustHeight();
    }
  }

  private adjustHeight(): void {
    const textarea: HTMLTextAreaElement = this.elementRef.nativeElement;

    if (!textarea.value && this.autoResizeValue.trim()) {
      textarea.value = this.autoResizeValue;
    }

    // Если поле очищено, сбрасываем высоту до исходной и скрываем скролл
    if (!textarea.value.trim() || !this.autoResizeValue.trim()) {
      this.renderer.setStyle(textarea, 'height', `${this.initialHeight}px`);
      this.renderer.setStyle(textarea, 'overflow-y', 'hidden');
      return;
    }

    // Сбрасываем высоту для корректного расчёта scrollHeight
    this.renderer.setStyle(textarea, 'height', `${this.initialHeight}px`);
    const scrollHeight = textarea.scrollHeight;

    // Если содержимое не превышает исходной высоты (то есть курсор остаётся на первой строке),
    // оставляем высоту равной исходной и скрываем скролл
    if (scrollHeight <= this.initialHeight + 1) {
      // Дополнительная погрешность в 1px может возникать из-за округлений – учитываем её
      this.renderer.setStyle(textarea, 'height', `${this.initialHeight}px`);
      this.renderer.setStyle(textarea, 'overflow-y', 'hidden');
      return;
    }

    // Определяем максимальную высоту для textarea (5 строк)
    const maxHeight = this.lineHeight * this.maxRows;
    const newHeight = Math.min(scrollHeight, maxHeight);
    this.renderer.setStyle(textarea, 'height', `${newHeight}px`);

    // Если фактическое содержимое превышает maxHeight, включаем вертикальный скролл
    if (scrollHeight > maxHeight) {
      this.renderer.setStyle(textarea, 'overflow-y', 'auto');
    } else {
      this.renderer.setStyle(textarea, 'overflow-y', 'hidden');
    }
  }
}
