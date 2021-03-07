import * as Preact from "preact";

import {State, Update} from "./state/State";

// Create an empty dispatcher until we fill in the actual proper dispatcher,
// which happens in index.tsx.
const initial: (action: Update) => void = (a) => {};

export default Preact.createContext(initial);

