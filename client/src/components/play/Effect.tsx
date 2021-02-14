import type {Property} from "../../model/read/Property";

export default function Effect(effect: Property) {
  let quality = effect.quality;
  let value = effect.value;

  switch (value.kind) {
    case "bool":
      if (value.value) {
        return <p class="fst-italic">Du erlangst {quality.name}.</p>;
      }
      else {
        return <p class="fst-italic">Du verlierst {quality.name}.</p>;
      }
    case "int":
      return (<p class="fst-italic">
        Du besitzt jetzt {value.value} {quality.name}.
      </p>);
    case "enum":
      return (<p class="fst-italic">
        {quality.name} ist jetzt: {value.value}.
      </p>);
  }
}
