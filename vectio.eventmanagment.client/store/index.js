export const state = () => ({
  networkError: null,
  loading: false,
  counter: 0
})

export const mutations = {
  SET_ERROR(state, error) {
    state.networkError = error
  },
  SET_LOADING(state, value) {
    state.loading = value
  },
  INC_COUNTER(state) {
    state.counter += 1
    state.loading = true
  },
  DEC_COUNTER(state) {
    if (state.counter > 0) state.counter -= 1
    if (state.counter === 0) state.loading = false
  }
}

export const actions = {
  setError({ commit }, error) {
    commit('SET_ERROR', error)
  },
  setLoading({ commit }, value) {
    commit('SET_LOADING', value)
  },
  startRequest({ commit }) {
    commit('INC_COUNTER')
  },
  endRequest({ commit }) {
    commit('DEC_COUNTER')
  }
}
