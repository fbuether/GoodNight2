import type {Property as State} from "../../model/read/Property";
import {QualityType} from "../../model/read/Quality";


export default function Effect(effect: State) {
  let quality = effect.quality;
  let value = effect.value;

  switch (quality.type) {
    case QualityType.Bool:
      if (value == "true") {
        return <p class="fst-italic">Du erlangst {quality.name}.</p>;
      }
      else {
        return <p class="fst-italic">Du verlierst {quality.name}.</p>;
      }
    case QualityType.Int:
      return (<p class="fst-italic">
        Du besitzt jetzt {value} {quality.name}.
      </p>);
    case QualityType.Enum:
      return (<p class="fst-italic">
        {quality.name} ist jetzt: {value}.
      </p>);
  }
}
