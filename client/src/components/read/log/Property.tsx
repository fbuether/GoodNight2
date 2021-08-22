import {Property as State} from "../../../state/model/read/Property";
import {QualityType} from "../../../state/model/read/Quality";

import Icon from "../../common/Icon";
import {isIcon} from "../../common/Icon";


export default function Property(property: State) {
  let quality = property.quality;
  let value = property.value;

  let iconName = quality.icon !== null && isIcon(quality.icon)
      ? quality.icon
      : "swap-bag" as const;

  let display = "";
  switch (quality.type) {
    case QualityType.Bool:
      display = value == "true"
          ? property.quality.name
          : "";
      break;

    case QualityType.Int:
      display = value + " " + property.quality.name;
      break;

    case QualityType.Enum:
      display = property.quality.name + ": " + value;
      break;
  }

  return (
    <li class="list-group-item text-truncate">
      <Icon name={iconName} class="mr-1" />
      {display}
    </li>
  );
}
