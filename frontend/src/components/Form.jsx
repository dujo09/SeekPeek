import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import axios from "axios";
import formFieldGroupsJSON from "./formFields.json";

const economicImpactFields = [];

const environmentalImpactFields = [];

const infrastructuralImpactFields = [];

export function Form() {
  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
  } = useForm({});
  const [loadingFields, setLoadingFields] = useState([]);

  useEffect(() => {
    console.log(formFieldGroupsJSON);
  }, []);

  async function onSubmit(data) {
    const answers = [];
    Object.keys(data).forEach(function (key, index) {
      answers.push({ question: key, answer: data[key] });
    });

    console.log(answers);
  }

  async function getAnswerSuggerstion(fieldLabel, fieldId) {
    setLoadingFields((prevLoadingFields) => [...prevLoadingFields, fieldId]);

    const question = "Odogovori na pitanje: " + fieldLabel;

    const response = await axios.post("http://127.0.0.1:5001/getAiResponse", {
      question,
    });
    const answer = response.data.answer.output;
    console.log("Response for getAnswerSuggestion: " + answer);

    setLoadingFields((prevLoadingFields) => {
      return prevLoadingFields.filter((field) => field !== fieldId);
    });
    return answer;
  }

  return (
    <div className="w-full">
      <p className="text-3xl uppercase text-center mb-5">Forma za prijavnicu</p>
      <form
        className="flex flex-col space-y-5 max-h-[90vh] max-w-4/5 mx-auto "
        onSubmit={handleSubmit(onSubmit)}
      >
        {formFieldGroupsJSON.map((formFieldGroup) => (
          <div
            className="mx-auto bg-slate-100 p-10 rounded-lg"
            key={formFieldGroup.id}
          >
            <p className="text-xl uppercase text-center">
              {formFieldGroup.fieldGroupLabel}
            </p>
            {formFieldGroup.fields.map((field) => (
              <div key={field.id} className="flex flex-col">
                <label className="text-md">{field.question}</label>
                <textarea
                  className="bg-blue-200 p-2 h-64 rounded-lg"
                  {...register(field.question)}
                />
                <button
                  disabled={loadingFields.includes(field.id)}
                  className="rounded-full bg-blue-500 w-fit px-5 my-2 mx-auto font-medium text-white disabled:bg-blue-300 hover:bg-blue-300 "
                  type="button"
                  onClick={async () =>
                    setValue(
                      field.question,
                      await getAnswerSuggerstion(field.question, field.id),
                    )
                  }
                >
                  {loadingFields.includes(field.id)
                    ? "Loading..."
                    : "Get suggestion"}
                </button>
                {errors[field.question] && (
                  <span className="text-red-600">Field is required!</span>
                )}
              </div>
            ))}
          </div>
        ))}
        <input
          className="bg-blue-500 p-2 font-medium text-white upercase  hover:bg-blue-300 "
          type="submit"
        />
      </form>
    </div>
  );
}
