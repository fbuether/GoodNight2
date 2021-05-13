import type {Property} from "./Property";


export interface Player {
  user: string;
  name: string;
  state: Array<Property>;
}
