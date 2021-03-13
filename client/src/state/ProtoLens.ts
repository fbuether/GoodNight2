
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

interface AddPrismProp<TOrigin, TObject> {
  prop<TKey extends string & keyof TObject, TThis>(
    this: TThis, key: TKey)
  : TThis & Record<TKey, Prism<TOrigin, TObject[TKey]>>;
}


type PrismBuilderFunction<
    TOrigin,
    TSubtype,
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
    builder: PrismBuilderFunction<TOrigin, TSubtype, TKey, TResult>)
  : TThis & Record<TKey, TResult>;
}


type LensBuilderFunction<
    TOrigin,
    TObject,
    TKey extends string & keyof TObject,
    TResult extends FullLens<TOrigin, TObject[TKey]>>
    = (id: FullLens<TOrigin, TObject[TKey]>) => TResult;

interface AddPath<TOrigin, TObject> {
  path<TKey extends string & keyof TObject,
      TResult extends FullLens<TOrigin, TObject[TKey]>,
      TThis>(
        this: TThis,
        key: TKey,
        builder: LensBuilderFunction<TOrigin, TObject, TKey, TResult>)
  : TThis & Record<TKey, TResult>;
}


interface AddPrismPath<TOrigin, TObject> {
  path<
      TKey extends string & keyof TObject,
      TResult extends FullPrism<TOrigin, TObject[TKey]>,
      TThis>(
        this: TThis,
        key: TKey,
        builder: PrismBuilderFunction<TOrigin, TObject[TKey], TKey, TResult>)
  : TThis & Record<TKey, TResult>;
}


type FullLens<O,V> = Lens<O,V> & AddProp<O,V> & AddUnion<O,V> & AddPath<O,V>;

type FullPrism<O,V> = Prism<O,V> & AddPrismProp<O,V> & AddUnion<O,V> & AddPrismPath<O,V>;


//------------------------------------------------------------------------------


function makeLens<TOrigin, TObject, TKey extends string & keyof TObject>(
  baseLens: Lens<TOrigin, TObject>,
  key: TKey)
: FullLens<TOrigin, TObject[TKey]> {
  return {
    get: (origin: TOrigin) => (baseLens.get(origin))[key],
    set: (value: TObject[TKey]) => (origin: TOrigin) => {
      console.log("set in makeLens,", key, value, origin);

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

    union: function<
        TSubKey extends string,
        TSubtype extends TObject[TKey],
        TResult extends FullPrism<TOrigin, TSubtype>,
        TThis>(
          this: TThis & FullLens<TOrigin, TObject[TKey]>,
          key: TSubKey,
          guard: Guard<TObject[TKey], TSubtype>,
          builder: PrismBuilderFunction<TOrigin, TSubtype, TSubKey, TResult>)
    : TThis & Record<TSubKey, TResult> {

      return addUnion<
          TOrigin,
          TObject[TKey],
          TSubKey,
          TSubtype,
          TResult,
          TThis>(
            this, key, guard, builder);
    },

    path: function<
        TSubKey extends string & keyof TObject[TKey],
        TResult extends FullLens<TOrigin, TObject[TKey][TSubKey]>,
        TThis>(
          this: TThis & FullLens<TOrigin, TObject[TKey]>,
          key: TSubKey,
          builder: LensBuilderFunction<TOrigin, TObject[TKey], TSubKey, TResult>)
    : TThis & FullLens<TOrigin, TObject[TKey]> & Record<TSubKey, TResult> {

      return addLensPath<
            TOrigin,
            TObject[TKey],
            TSubKey,
            TResult,
            TThis & FullLens<TOrigin, TObject[TKey]>>(
              this, key, builder);
    }
  };
}


function makePrism<TOrigin, TObject, TSubtype extends TObject>(
  baseLens: Prism<TOrigin, TObject>,
  guard: Guard<TObject, TSubtype>)
: FullPrism<TOrigin, TSubtype> {
  return {
    get: (origin: TOrigin): TSubtype | null => {
      let obj = baseLens.get(origin);
      return obj != null && guard(obj) ? obj : null;
    },
    set: (value: TSubtype) => (origin: TOrigin): TOrigin => {
      console.log("set in prism", value, origin);

      let obj = baseLens.get(origin);
      return obj != null && guard(obj) ? baseLens.set(value)(origin) : origin;
    },

    prop: function<TKey extends string & keyof TSubtype, TThis>(
      this: TThis & FullPrism<TOrigin, TSubtype>,
      key: TKey) {
      return addPrismProp<
          TKey,
          TOrigin,
          TSubtype,
          TThis>(
            this, key);
    },

    union: function<
        TKey extends string,
        TSubSubtype extends TSubtype,
        TResult extends FullPrism<TOrigin, TSubSubtype>,
        TThis>(
          this: TThis & Prism<TOrigin, TSubtype>,
          key: TKey,
          guard: Guard<TSubtype, TSubSubtype>,
          builder: PrismBuilderFunction<TOrigin, TSubSubtype, TKey, TResult>)
    : TThis & Record<TKey, TResult> {

      return addUnion<
          TOrigin,
          TSubtype,
          TKey,
          TSubSubtype,
          TResult,
          TThis>(
            this, key, guard, builder);
    },

    path: function<
        TKey extends string & keyof TSubtype,
        TResult extends FullPrism<TOrigin, TSubtype[TKey]>,
        TThis
        >(
          this: TThis & Prism<TOrigin, TSubtype>,
          key: TKey,
          builder: PrismBuilderFunction<TOrigin, TSubtype[TKey], TKey, TResult>)
    : TThis & Record<TKey, TResult> {
      return addPrismPath<
          TOrigin,
          TSubtype,
          TKey,
          TResult,
          TThis>(
            this, key, builder);
    }
  };
}


function makePrismFromProp<TOrigin, TObject,
  TKey extends string & keyof TObject>(
    baseLens: Prism<TOrigin, TObject>,
    key: TKey)
: FullPrism<TOrigin, TObject[TKey]> {
  return {
    get: (origin: TOrigin): TObject[TKey] | null => {
      let obj = baseLens.get(origin);
      return obj != null ? obj[key] : null;
    },
    set: (value: TObject[TKey]) => (origin: TOrigin): TOrigin => {
      console.log("set in makePrism", value, origin);

      let obj = baseLens.get(origin);
      return obj != null ? baseLens.set({ ...obj, [key]: value })(origin) : origin;
    },

    prop: function<TSubKey extends string & keyof TObject[TKey], TThis>(
      this: TThis & Prism<TOrigin, TObject[TKey]>,
      key: TSubKey) {
      return addPrismProp<
          TSubKey,
          TOrigin,
          TObject[TKey],
          TThis>(
            this, key);
    },

    union: function<
        TSubKey extends string,
        TSubtype extends TObject[TKey],
        TResult extends FullPrism<TOrigin, TSubtype>,
        TThis>(
          this: TThis & Prism<TOrigin, TSubtype>,
          key: TSubKey,
          guard: Guard<TObject[TKey], TSubtype>,
          builder: PrismBuilderFunction<TOrigin, TSubtype, TSubKey, TResult>)
    : TThis & Record<TSubKey, TResult> {

      return addUnion<
          TOrigin,
          TSubtype,
          TSubKey,
          TSubtype,
          TResult,
          TThis>(
            this, key, guard, builder);
    },

    path: function<
        TSubKey extends string & keyof TObject[TKey],
        TResult extends FullPrism<TOrigin, TObject[TKey][TSubKey]>,
        TThis
        >(
          this: TThis & Prism<TOrigin, TObject[TKey]>,
          key: TSubKey,
          builder: PrismBuilderFunction<TOrigin, TObject[TKey][TSubKey], TSubKey, TResult>)
    : TThis & Record<TSubKey, TResult> {
      return addPrismPath<
          TOrigin,
          TObject[TKey],
          TSubKey,
          TResult,
          TThis>(
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

  let propLens: Lens<TOrigin, TObject[TKey]> = {
    get: (origin: TOrigin) => (baseLens.get(origin))[key],
    set: (value: TObject[TKey]) => (origin: TOrigin) => {
      return baseLens.set({ ...baseLens.get(origin), [key]: value })(origin);
    }
  };

  let propLensObj = {[key]: propLens} as Record<TKey, Lens<TOrigin, TObject[TKey]>>;
  return {
    ...baseLens,
    ...propLensObj
  };
}


function addPrismProp<TKey extends string,
    TOrigin,
    TObject extends Record<TKey, TObject[TKey]>,
    TThis>(
      basePrism: TThis & Prism<TOrigin, TObject>,
      key: TKey)
: TThis & Record<TKey, Prism<TOrigin, TObject[TKey]>> {

  let propPrism: Prism<TOrigin, TObject[TKey]> = {
    get: (origin: TOrigin) => {
      let base = basePrism.get(origin);
      return base != null ? base[key] : null;
    },
    set: (value: TObject[TKey]) => (origin: TOrigin) => {
      let base = basePrism.get(origin);
      return base != null
          ? basePrism.set({ ...base, [key]: value })(origin)
          : origin;
    }
  };

  let propPrismObj = {[key]: propPrism} as Record<TKey, Prism<TOrigin, TObject[TKey]>>;
  return {
    ...basePrism,
    ...propPrismObj
  };
}



function addUnion<
    TOrigin,
    TObject,
    TKey extends string,
    TSubtype extends TObject,
    TResult extends FullPrism<TOrigin, TSubtype>,
    TThis>(
      baseLens: TThis & Prism<TOrigin, TObject>,
      key: TKey,
      guard: Guard<TObject, TSubtype>,
      builder: PrismBuilderFunction<TOrigin, TSubtype, TKey, TResult>)
: TThis & Record<TKey, TResult> {

  let keyPrism: FullPrism<TOrigin, TSubtype> = makePrism(baseLens, guard);
  let unionPrism = builder(keyPrism);
  let unionPrismObj = {[key]: unionPrism} as Record<TKey, TResult>;
  return {
    ...baseLens,
    ...unionPrismObj
  };
}


function addLensPath<
    TOrigin,
    TObject,
    TKey extends string & keyof TObject,
    TResult extends FullLens<TOrigin, TObject[TKey]>,
    TThis>(
  baseLens: TThis & Lens<TOrigin, TObject>,
  key: TKey,
  builder: LensBuilderFunction<TOrigin, TObject, TKey, TResult>)
: TThis & Record<TKey, TResult> {

  let keyLens = makeLens<TOrigin, TObject, TKey>(baseLens, key);
  let pathLens: TResult = builder(keyLens);
  let pathLensObj = {[key]: pathLens} as Record<TKey, TResult>;
  return {
    ...baseLens,
    ...pathLensObj
  };
}


function addPrismPath<
    TOrigin,
    TObject,
    TKey extends string & keyof TObject,
    TResult extends FullPrism<TOrigin, TObject[TKey]>,
    TThis>(
  basePrism: TThis & Prism<TOrigin, TObject>,
  key: TKey,
  builder: PrismBuilderFunction<TOrigin, TObject[TKey], TKey, TResult>)
: TThis & Record<TKey, TResult> {

  let keyPrism = makePrismFromProp<TOrigin, TObject, TKey>(
    basePrism, key);
  let pathPrism: TResult = builder(keyPrism);
  let pathPrismObj = {[key]: pathPrism} as Record<TKey, TResult>;
  return {
    ...basePrism,
    ...pathPrismObj
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


  union = function<
      TKey extends string,
      TSubtype extends TObject,
      TResult extends FullPrism<TObject, TSubtype>,
      TThis>(
        this: TThis & Lens<TObject, TObject>,
        key: TKey,
        guard: Guard<TObject, TSubtype>,
        builder: PrismBuilderFunction<TObject, TSubtype, TKey, TResult>)
  : TThis & Record<TKey, TResult> {

    return addUnion<
        TObject,
        TObject,
        TKey,
        TSubtype,
        TResult,
        TThis>(
          this, key, guard, builder);
  }


  path = function<
      TKey extends string & keyof TObject,
      TResult extends FullLens<TObject, TObject[TKey]>,
      TThis>(
    this: TThis & Lens<TObject, TObject>,
    key: TKey,
    builder: LensBuilderFunction<TObject, TObject, TKey, TResult>) {
    return addLensPath<
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

// let pgLens = id<Pag>().union("hp", guardHP, lens => lens.prop("message"));


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
// console.log(staLens.user.set("wurst")(sta));
// console.log(staLens.page.set({ kind: "RP" as const, story: "my-story" })(sta));
console.log("sup", staLens.page.asHP.message.greeting.set("wassup")(sta));
