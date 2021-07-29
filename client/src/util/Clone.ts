
export function clone<TObj extends object>(
  obj: TObj, assigns: object)
: TObj
{
  var sameClassObject = Object.create(Object.getPrototypeOf(obj));
  let updated = Object.assign(sameClassObject, obj, assigns);
  return updated;
}
