
export async function noEarlierThan<T>(milliseconds: number, f: Promise<T>)
: Promise<T> {
  let delay = new Promise(r => setTimeout(r, milliseconds));
  let [result, _] = await Promise.all([f, delay]);
  return result;
}
