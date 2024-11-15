from collections import OrderedDict
from functools import wraps 


class DotMap(dict):

    """
      dot access of python dictionary
    """

    def __setstate__(self, state):
        self.update(state)
        self.__dict__ = self

    def __init__(self, *args, **kwargs):
        super(DotMap, self).__init__(*args, **kwargs)
        for arg in args:
            if isinstance(arg, dict):
                for k, v in arg.items():
                    if isinstance(v, dict):
                        v = DotMap(v)
                    if isinstance(v, list):
                        self.__typecast(v)
                    self[k] = v

        if kwargs:
            for k, v in kwargs.items():
                if isinstance(v, dict):
                    v = DotMap(v)
                elif isinstance(v, list):
                    self.__typecast(v)
                self[k] = v

    def __typecast(self, v_list):
        for i, element in enumerate(v_list):
            if isinstance(element, dict):
                v_list[i] = DotMap(element)
            elif isinstance(element, list):
                self.__typecast(element)

    def __getstate__(self):
        return self

    def __getattr__(self, attr):
        return self.get(attr, '')

    # def __setattr__(self, key, value):
    #     self.__setitem__(key, value)
    __setattr__ = dict.__setitem__
    __delattr__ = dict.__delitem__

    def __setitem__(self, key, value):
        super(DotMap, self).__setitem__(key, value)
        self.__dict__.update({key: value})

    # def __delattr__(self, item):
    #     self.__delitem__(item)

    def __delitem__(self, key):
        super(DotMap, self).__delitem__(key)
        del self.__dict__[key]


class OrderedDotMap(OrderedDict):

    """
      dot access of python OrderedDict
    """

    def __setstate__(self, state):
        self.update(state)
        self.__dict__ = self

    def __init__(self, *args, **kwargs):
        super(OrderedDotMap, self).__init__(*args, **kwargs)
        for arg in args:
            if isinstance(arg, dict):
                for k, v in arg.items():
                    if isinstance(v, dict):
                        v = DotMap(v)
                    if isinstance(v, list):
                        self.__typecast(v)
                    self[k] = v

        if kwargs:
            for k, v in kwargs.items():
                if isinstance(v, dict):
                    v = DotMap(v)
                elif isinstance(v, list):
                    self.__typecast(v)
                self[k] = v

    def __typecast(self, v_list):
        for i, element in enumerate(v_list):
            if isinstance(element, dict):
                v_list[i] = DotMap(element)
            elif isinstance(element, list):
                self.__typecast(element)

    def __getstate__(self):
        return self

    def __getattr__(self, attr):
        return self.get(attr, '')

    # def __setattr__(self, key, value):
    #     self.__setitem__(key, value)
    __setattr__ = dict.__setitem__
    __delattr__ = dict.__delitem__

    def __setitem__(self, key, value):
        super(OrderedDotMap, self).__setitem__(key, value)
        self.__dict__.update({key: value})

    # def __delattr__(self, item):
    #     self.__delitem__(item)

    def __delitem__(self, key):
        super(OrderedDotMap, self).__delitem__(key)
        del self.__dict__[key]


def dotmap(func):
    @wraps(func)
    def converter(in_dict: dict):
        dotMap = DotMap(in_dict)
        return func(dotMap)
    return converter


def ordered_dotmap(func):
    @wraps(func)
    def converter(in_dict: dict):
        dotMap = OrderedDotMap(in_dict)
        return func(dotMap)
    return converter