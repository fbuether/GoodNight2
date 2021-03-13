
interface Prism<TObject, TValue> {
  get: (obj: TObject) => TValue | null;
  set: (value: TValue) => (obj: TObject) => TObject;
}

interface Lens<TObject, TValue> extends Prism<TObject, TValue> {
  get: (obj: TObject) => TValue;
}


interface AddProp<TOrigin, TObject> {
  prop<TKey extends string & keyof TObject, TThis>(
    this: TThis,
    key: TKey)
  : TThis & Record<TKey, Lens<TOrigin, TObject[TKey]>>;
}


type UnionBuilderFunction<
    TOrigin,
    TObject,
    TSubtype extends TObject,
    TKey extends string,
    TResult extends FullPrism<TOrigin, TSubtype>>
    = (id: FullPrism<TOrigin, TSubtype>) => TResult;

type Guard<TObject, TSubtype extends TObject> = (a: TObject) => a is TSubtype;

interface AddUnion<TOrigin, TObject> {
  union<TKey extends string,
      TSubtype extends TObject,
      TResult extends FullPrism<TOrigin, TSubtype>,
      TThis>(
    this: TThis,
    key: TKey,
    guard: Guard<TObject, TSubtype>,
    builder: UnionBuilderFunction<TOrigin, TObject, TSubtype, TKey, TResult>)
  : TThis & Record<TKey, Prism<TOrigin, TSubtype>>;
}


type PathBuilderFunction<
    TOrigin,
    TObject,
    TKey extends string & keyof TObject,
    TResult extends FullLens<TOrigin, TObject[TKey]>>
    = (id: FullLens<TOrigin, TObject[TKey]>) => TResult;

interface AddPath<TOrigin, TObject> {
  path<TKey extends string & keyof TObject,
      TThis,
      TResult extends FullLens<TOrigin, TObject[TKey]>>(
    this: TThis,
    key: TKey,
    builder: PathBuilderFunction<TOrigin, TObject, TKey, TResult>)
  : TThis & Record<TKey, TResult>;
}


type FullLens<O,V> = Lens<O,V> & AddProp<O,V> & AddUnion<O,V> & AddPath<O,V>;

type FullPrism<O,V> = Prism<O,V> & AddProp<O,V> & AddUnion<O,V> & AddPath<O,V>;


//------------------------------------------------------------------------------


function makeLens<TOrigin, TObject, TKey extends string & keyof TObject>(
  baseLens: Lens<TOrigin, TObject>,
  key: TKey)
: FullLens<TOrigin, TObject[TKey]> {
  return {
    get: (origin: TOrigin) => (baseLens.get(origin))[key],
    set: (value: TObject[TKey]) => (origin: TOrigin) => {
      return baseLens.set({ ...baseLens.get(origin), [key]: value })(origin);
    },

    prop: function<TSubKey extends string & keyof TObject[TKey], TThis>(
      this: TThis & FullLens<TOrigin, TObject[TKey]>,
      key: TSubKey) {

      return addProp<
          TSubKey,
          TOrigin,
          TObject[TKey] & Record<TSubKey, TObject[TKey][TSubKey]>,
          TThis>(
            this, key);
    },

    union: function<TSubKey extends string, TSubtype extends TObject[TKey], TThis>(
      this: TThis & FullLens<TOrigin, TObject[TKey]>,
      key: TSubKey,
      guard: Guard<TObject[TKey], TSubtype>)
    : TThis & Record<TSubKey, Prism<TOrigin, TSubtype>> {

      return addUnion<
          TOrigin,
          TObject[TKey],
          TSubKey,
          TSubtype,
          TThis>(
            this, key, guard);
    },

    path: function<
        TSubKey extends string & keyof TObject[TKey],
        TResult extends FullLens<TOrigin, TObject[TKey][TSubKey]>,
        TThis>(
          this: TThis & FullLens<TOrigin, TObject[TKey]>,
          key: TSubKey,
          builder: PathBuilderFunction<TOrigin, TObject[TKey], TSubKey, TResult>)
    : TThis & FullLens<TOrigin, TObject[TKey]> & Record<TSubKey, TResult> {

      return addPath<
            TOrigin,
            TObject[TKey],
            TSubKey,
            TResult,
            TThis & FullLens<TOrigin, TObject[TKey]>>(
              this, key, builder);
    }
  };
}


function addProp<TKey extends string,
    TOrigin,
    TObject extends Record<TKey, TObject[TKey]>,
    TThis>(
  baseLens: TThis & Lens<TOrigin, TObject>,
  key: TKey)
: TThis & Record<TKey, Lens<TOrigin, TObject[TKey]>> {

  let propLens: Lens<TOrigin, TObject[TKey]> = makeLens(baseLens, key);
  let propLensObj = {[key]: propLens} as Record<TKey, Lens<TOrigin, TObject[TKey]>>;
  return {
    ...baseLens,
    ...propLensObj
  };
}


function addUnion<
    TOrigin,
    TObject,
    TKey extends string,
    TSubtype extends TObject,
    TThis>(
  baseLens: TThis & Lens<TOrigin, TObject>,
  key: TKey,
  guard: Guard<TObject, TSubtype>)
: TThis & Record<TKey, Prism<TOrigin, TSubtype>> {

  let unionPrism = {
    get: (origin: TOrigin) => {
      let obj = baseLens.get(origin);
      return guard(obj) ? obj : null;
    },
    set: (value: TSubtype) => (origin: TOrigin) => {
      let obj = baseLens.get(origin);
      return guard(obj) ? baseLens.set(obj)(origin) : origin;
    }
  };

  let unionPrismObj = {[key]: unionPrism} as Record<TKey, Prism<TOrigin, TSubtype>>;
  return {
    ...baseLens,
    ...unionPrismObj
  };
}


function addPath<
    TOrigin,
    TObject,
    TKey extends string & keyof TObject,
    TResult extends FullLens<TOrigin, TObject[TKey]>,
    TThis>(
  baseLens: TThis & Lens<TOrigin, TObject>,
  key: TKey,
  builder: PathBuilderFunction<TOrigin, TObject, TKey, TResult>)
: TThis & Record<TKey, TResult> {

  let keyLens = makeLens<TOrigin, TObject, TKey>(baseLens, key);
  let pathLens: TResult = builder(keyLens);
  let pathLensObj = {[key]: pathLens} as Record<TKey, TResult>;
  return {
    ...baseLens,
    ...pathLensObj
  };
}




type PropThisRestriction<TObject, TKey extends string & keyof TObject> =
    Lens<TObject & Record<TKey, TObject[TKey]>,
         TObject & Record<TKey, TObject[TKey]>>;

class IdLens<TObject> implements FullLens<TObject, TObject> {
  get = (base: TObject) => base;
  set = (newBase: TObject) => (oldBase: TObject) => newBase;

  prop = function<TKey extends string & keyof TObject, TThis>(
    this: TThis & PropThisRestriction<TObject, TKey>,
    key: TKey) {

    return addProp<TKey,
        TObject & Record<TKey, TObject[TKey]>,
        TObject & Record<TKey, TObject[TKey]>,
        TThis>(
          this, key);
  }


  union = function<TKey extends string, TSubtype extends TObject, TThis>(
    this: TThis & Lens<TObject, TObject>,
    key: TKey,
    guard: Guard<TObject, TSubtype>) {

    return addUnion<
        TObject,
        TObject,
        TKey,
        TSubtype,
        TThis>(
          this, key, guard);
  }


  path = function<
      TKey extends string & keyof TObject,
      TResult extends FullLens<TObject, TObject[TKey]>,
      TThis>(
    this: TThis & Lens<TObject, TObject>,
    key: TKey,
    builder: PathBuilderFunction<TObject, TObject, TKey, TResult>) {
    return addPath<
        TObject,
        TObject,
        TKey,
        TResult,
        TThis
        >(
      this, key, builder);
  }
}


export function id<S>(): IdLens<S>
{
  return new IdLens<S>();
}




//------------------


interface OnlyNumber {
  n: number;
}

interface HPM {
  greeting: string;
  count: OnlyNumber;
}


interface HP {
  kind: "HP";
  message: HPM;
}

interface RP {
  kind: "RP";
  story: string;
}


type Pag = HP | RP;

const guardHP = (a: Pag): a is HP => (a.kind == "HP");
const guardRP = (a: Pag): a is RP => (a.kind == "RP");

interface Sta {
  page: Pag;
  user: string;
}


let hpLens = id<HP>().path("message", lens =>
    lens.path("count", lens => lens));


let staLens = id<Sta>().prop("user")
  .path("page", lens =>
      lens.union("asHP", guardHP, lens =>
          lens.path("message", lens =>
              lens.prop("greeting").prop("count")))
        .union("asRP", guardRP, lens =>
            lens.prop("story")));


let sta: Sta = {
  user: "baba",
  page: {
    kind: "HP" as const,
    message: {
      greeting: "hello",
      count: {
        n: 4
      }
    }
  }
};


console.log(sta);
console.log(staLens.user.set("wurst")(sta));
console.log(staLens.page.set({ kind: "RP" as const, story: "my-story" })(sta));
console.log(staLens.page.message.greeting.set("wassup")(sta));
