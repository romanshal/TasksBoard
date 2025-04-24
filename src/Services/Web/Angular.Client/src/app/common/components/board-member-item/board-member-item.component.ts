import { Component, Input } from '@angular/core';
import { BoardMemberModel } from '../../models/board-member/board-member.model';
import { BoardPermission } from '../../models/board-permission/board-permission.model';

@Component({
  selector: 'app-board-member-item',
  standalone: false,
  templateUrl: './board-member-item.component.html',
  styleUrl: './board-member-item.component.scss'
})
export class BoardMemberItemComponent {
  @Input() member!: BoardMemberModel;
  @Input() permissions!: BoardPermission[];
  @Input() userId!: string;
  @Input() backgroundColor!: string;

  private openedSettingsId?: string;

  openSettings(id: string) {
    if (this.openedSettingsId) {
      let openedSettings = document.getElementById(this.openedSettingsId);

      if (openedSettings) {
        if (this.openedSettingsId === id) {
          openedSettings.style.display = "none";
          this.openedSettingsId = undefined;

          return;
        }

        openedSettings.style.display = "none";
      }
    }

    let settings = document.getElementById(id);
    if (!settings) {
      return;
    }

    this.openedSettingsId = id;

    settings.style.display = "block";
  }

  isPermissionsCheck(permissionId: string) {
    if (this.member.Permissions.findIndex(p => p.BoardPermissionId === permissionId) !== -1) {
      return true;
    } else {
      return false;
    }
  }
}
