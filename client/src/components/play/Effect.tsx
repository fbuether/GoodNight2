import type {Property} from "../../model/read/Property";

export default function Effect(effect: Property) {
  let quality = effect.quality;
  let value = effect.value;

  switch (value.kind) {
    case "bool":
      if (value.value) {
        return <div key={quality.name}>Du erlangst {quality.name}.</div>;
      }
      else {
        return <div key={quality.name}>Du verlierst {quality.name}.</div>;
      }
    case "int":
      return (<p class="fst-italic">
        Du besitzt jetzt {value.value} {quality.name}.
      </p>);
    case "enum":
      return (<p>
        {quality.name} ist jetzt: {value.value}.
      </p>);
  }
}
