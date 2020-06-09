export default function(context) {
  const isAdmin =
    context.$auth.loggedIn &&
    context.$auth.user.roles.filter((x) => x === 'administrator').length

  if (!isAdmin) {
    return context.redirect('/auth/signIn')
  }
}
