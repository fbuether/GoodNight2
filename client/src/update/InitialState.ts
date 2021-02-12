import type State from "../model/State";

import {ofHref} from "../model/Page";


export default function initialState(url: URL): State {
  return {
    page: ofHref(url),
    user: "Mrs. Hollywookle"
  };
}
