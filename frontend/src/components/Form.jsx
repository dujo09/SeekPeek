import { useState } from "react";
import { useForm } from "react-hook-form";

const questions = [
  "Podignuta razina kvalitete javnih usluga (Poboljšava zadovljstvo građana i njihovu interakciju s javnim sektorom)",
  "Stvaranje novih radnih mjesta (Povećana zaposlenost, smanjuje, socijalnu nejednakost i doprinosi ekonomskom blagostanju)",
  "Poboljšana mogućnost sudjelovanja u procesima javne uprave (Omogućuje demokratičniji pristup odlučivanju i povećava transparentnost)",
  "Podignuta razina kvalitete života (Sveukupno poboljšanje uvjeta života kroz bolje usluge, javni prostor i sigurnost)",
  "Podignuta razina sigurnosti (Smanjuje rizike i povećava osjećaj sigurnosti građana)",
  "Podignuta razina podrške ranjivim skupinama (Promiče socijalnu jednakost i inkluziju)",
  "Jačanje digitalne pismenosti i tehnologije (Potiče korištenje suvremenih tehnologija za svakodnevni život i poslovanje)",
  "Promicanje rodne ravnopravnosti (Osigurava jednake prilike za sve skupine u društvu)",
  "Smanjenje socijalne isključenosti (Omogućuje pristup resursima i uslugama marginaliziranim skupinama)",
  "Smanjenje troškova života (Kroz učinkovitiju infrastrukturu i javne usluge smanjuje financijsko opterećenje građana)",
  "Razvoj lokalne zajednice (Osnažuje koheziju i povezanost unutar zajednice)",
];

export function Form() {
  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
  } = useForm({});
  async function onSubmit(data) {
    console.log(data);
  }

  async function getAnswerSuggerstion(question) {
    return "Odgovor na: " + question;
  }

  return (
    <div className="w-[95vw] mx-auto">
      <p className="text-3xl uppercase text-center">Forma</p>
      <form
        className="flex flex-col space-y-5 max-h-[90vh] overflow-y-scroll"
        onSubmit={handleSubmit(onSubmit)}
      >
        {questions.map((question) => (
          <div key={question} className="flex flex-col">
            <label className="text-md">{question} </label>
            <input
              className="bg-slate-200 p-2"
              {...register(question, { required: true })}
            />
            <button
              className="rounded-full bg-blue-500 w-fit px-5 my-2 mx-auto font-medium text-white"
              type="button"
              onClick={async () =>
                setValue(question, await getAnswerSuggerstion(question))
              }
            >
              Get suggestion
            </button>
            {errors[question] && (
              <span className="text-red-600">Field is required!</span>
            )}
          </div>
        ))}

        <input
          className="bg-blue-500 p-2 font-medium text-white upercase"
          type="submit"
        />
      </form>
    </div>
  );
}
