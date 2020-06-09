import Vue from 'vue'

Vue.filter('date', function(value) {
  if (!value) return ''
  return value.substring(0, 16).replace('T', ' ')
})
