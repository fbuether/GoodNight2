import * as Preact from "preact";

import {Update} from "./update/Update";
import State from "./model/State";

// Create an empty dispatcher until we fill in the actual proper dispatcher,
// which happens in index.tsx.
const initial: (action: Update) => void = (a) => {};

export default Preact.createContext(initial);

