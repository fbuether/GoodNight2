
export function requireObject(obj: any): object {
  if (typeof obj != "object") {
    console.error("Invalid object", obj);
    throw new Error("Invalid object");
  }

  return obj as object;
}


export function requireString(obj: any): string {
  if (typeof obj != "string") {
    console.error("Invalid string", obj);
    throw new Error("Invalid string");
  }

  return obj as string;
}


export function requireNumber(obj: any): number {
  if (typeof obj != "number") {
    console.error("Invalid number", obj);
    throw new Error("Invalid number");
  }

  return obj as number;
}


export function requireBoolean(obj: any): boolean {
  if (typeof obj != "boolean") {
    console.error("Invalid boolean", obj);
    throw new Error("Invalid boolean");
  }

  return obj as boolean;
}


export function optionalString(obj: any, prop: string): string | undefined {
  return Object.keys(obj).includes(prop) && obj[prop] != null
      ? requireString(obj[prop])
      : undefined;
}


export function deserialiseArray<T>(obj: any, prop: string,
  converter: (val: any) => T): Array<T> {
  return Array.isArray(obj[prop])
      ? obj[prop].map(converter)
      : new Array<T>()
}
