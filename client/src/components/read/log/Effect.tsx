import type {Property as State} from "../../../state/model/read/Property";
import {QualityType} from "../../../state/model/read/Quality";

import Icon from "../../common/Icon";
import {isIcon} from "../../common/Icon";


export default function Effect(effect: State) {
  let quality = effect.quality;
  let value = effect.value;

  let icon = effect.quality.icon !== null && isIcon(effect.quality.icon)
      ? <Icon name={effect.quality.icon} />
      : <></>;

  switch (quality.type) {
    case QualityType.Bool:
      if (value == "true") {
        return <p class="fst-italic">Du erlangst {icon} {quality.name}.</p>;
      }
      else {
        return <p class="fst-italic">Du verlierst {icon} {quality.name}.</p>;
      }
    case QualityType.Int:
      return (<p class="fst-italic">
        Du besitzt jetzt {value} {icon} {quality.name}.
      </p>);
    case QualityType.Enum:
      return (<p class="fst-italic">
        {icon} {quality.name} ist jetzt: {value}
      </p>);
  }
}
