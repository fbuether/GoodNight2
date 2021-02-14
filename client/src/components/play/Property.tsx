import Icon, {IconName} from "../play/Icon";

import type {Quality, QualityType} from "../../model/read/Quality";
import type {Value} from "../../model/read/Value";

import type {Property} from "../../model/read/Property";



export default function Property(property: Property) {
  // let text = <span>{quality.quality.name}</span>;

  // switch (quality.quality.type) {
  //   case QualityType.Bool:
  //     if (value.value) {
  //       text = <span>{quality.has} {quality.name}</span>;
  //     }
  //     break;
  //   case "enum":
  //     text = <span>{quality.name} <small title={quality.has}>({quality.has})</small></span>;
  //     break;
  // }

  // return (
  //   <li class="list-group-item text-truncate">
  //     <Icon name={quality.icon} />{" "}{text}
  //   </li>
  // );

  return (
    <li class="list-group-item text-truncate">
      {property.quality.name}: {property.value.value}
    </li>
  );
}
