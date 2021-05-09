import {Loadable, LoadableP} from "../../state/Loadable";

import Loading from "./Loading";
import Error from "./Error";


export default function LoadableLoader<P,T>(loadable: LoadableP<P,T> | Loadable<T>,
  onLoaded: (value: T) => JSX.Element) {

  switch (loadable.state) {
    case "unloaded":
    case "unloadedP":
    case "loading":
      return <Loading />;

    case "failed":
      return <Error message={loadable.error} />;

    case "loaded":
      return onLoaded(loadable.result);
  }
}
