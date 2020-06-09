export default function({ $axios, store }) {
  $axios.onRequest(() => {
    store.dispatch('startRequest')
  })

  $axios.onResponse(() => {
    store.dispatch('endRequest')
  })
  $axios.onError(() => {
    store.dispatch('endRequest')
  })
}
