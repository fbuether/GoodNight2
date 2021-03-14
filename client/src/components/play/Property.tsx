import Icon, {IconName, isIcon} from "../common/Icon";

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

  let icon = typeof property.quality.icon == "string"
      && isIcon(property.quality.icon)
      ? <Icon name={property.quality.icon} />
      : <Icon name="swap-bag" />;

  switch (property.value.kind) {
    case "int":
      return (
        <li class="list-group-item text-truncate">
          {icon} {property.value.value} {property.quality.name}
        </li>
      );
    case "bool":
      if (property.value.value) {
        return (
          <li class="list-group-item text-truncate">
            {icon} {property.quality.name}
          </li>
        );
      }
      else {
        return <></>;
      }
    case "enum":
      return (
        <li class="list-group-item text-truncate">
          {icon} {property.quality.name}
        </li>
      );
  }
}
