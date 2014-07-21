App.directive('tree', function($compile){
  return {
    restrict: 'E',
    replace: true,
    scope: {
      collection: '=',
      search: '=',
      version: '@version'
    },
    template: '<ul class="nav nav-stacked">'
            + '   <tree-item ng-repeat="item in collection | filter:search" item="item" version="//version//"></tree-item>'
            + '</ul>',
    link: function(scope, element, attrs){
        $compile(element.contents())(scope);
    }
  }
});

App.directive('treeItem', function($compile)
{
  function getTemplate()
  {
    var template = '';

    switch (DEV)
    {
      default: 
      case true:
        template = '<li>'
                 + '   <a ng-hide="item.value | isArray" href="' + rootPath + 'index.php?lang=en&page=index&module=docs&component=//item.value//">//item.key//</a>'
                 + '   <a ng-show="item.value | isArray" href="#">//item.key//</a>'
                 + '</li>';
        break;

      case false:
        template = '<li>'
                 + '   <a ng-hide="item.value | isArray" href="component.//item.value//.html?v=//version//">//item.key//</a>'
                 + '   <a ng-show="item.value | isArray" href="#">//item.key//</a>'
                 + '</li>';
        break;
    }

    return template;
  };

  return {
    restrict: 'E',
    replace: true,
    scope: {
      item: '=',
      version: '@version'
    },
    template: getTemplate(),
    link: function(scope, element, attrs){
      if (angular.isArray(scope.item.value)){
        element.append('<tree collection="item.value" version="//version//"></tree>');
      }
      $compile(element.contents())(scope);
    }
  }
});