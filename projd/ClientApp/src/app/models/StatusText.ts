import { Pipe, PipeTransform } from '@angular/core';

@Pipe({name: 'statusText'})
export class StatusTextPipe implements PipeTransform {
  public statusTexts = {
    0: 'New',
    5: 'Rejected',
    10: 'Submitted',
    20: 'Approved',
  }

  transform(value: number): string {
    return this.statusTexts[value];
  }
}
