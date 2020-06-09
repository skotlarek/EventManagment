const required = (propertyType) => {
  return (v) => (v && v.length > 0) || `Pole jest wymagane`
}
const minLength = (propertyType, minLength) => {
  return (v) =>
    (v && v.length >= minLength) ||
    `Pole musi zawierać co najmniej ${minLength} znaków`
}
const minLengthOrEmpty = (propertyType, minLength) => {
  return (v) =>
    !v ||
    (v && v.length >= minLength) ||
    `Pole musi być puste albo zawierać co najmniej ${minLength} znaków`
}
const maxLength = (propertyType, maxLength) => {
  return (v) =>
    (v && v.length <= maxLength) ||
    `Pole może zawierać co najwyżej ${maxLength} znaków`
}

const emailFormat = () => {
  // eslint-disable-next-line no-useless-escape
  const regex = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,24})+$/
  return (v) => (v && regex.test(v)) || 'Wymagany jest poprawny adres email'
}

export default {
  required,
  minLength,
  minLengthOrEmpty,
  maxLength,
  emailFormat
}
