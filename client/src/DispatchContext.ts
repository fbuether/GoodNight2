import * as Preact from "preact";

import {State} from "./state/State";
import {DispatchAction} from "./state/Dispatch";

// Create an empty dispatcher until we fill in the actual proper dispatcher,
// which happens in index.tsx.
const initial: (action: DispatchAction) => void = (a) => {};

export default Preact.createContext(initial);

