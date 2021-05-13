
import Icon from "./Icon";


export interface SaveButton {
  isSaving: boolean;
}

export default function SaveButton(state: SaveButton) {
  return state.isSaving
      ? <div type="submit" class="btn btn-primary disabled loading">
          <Icon name="empty-hourglass" class="mr-2" />
          Speichere…
        </div>
      : <button type="submit" class="btn btn-primary">
          <Icon name="save" class="mr-2" />
          Speichern
        </button>;
}
